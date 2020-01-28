...
<div class="main-middle-wrapper">
	<table class="table">
		<thead>
			<tr>
				<th>Laskunumero</th>
				<th>Projektin nimi</th>
				<th>Tuntiveloitteinen</th>
				<th>Loppuhinta</th>
				<th>Tuntihinta</th>
				<th>Asiakas</th>
				<th>Aloituspäivämäärä</th>
				<th>Toiminnot</th>
			</tr>
		</thead>
		<tbody>
			<tr>
				<form action="./includes/add-project.php" method="GET">
					<td><input class="modify" type="number" name="number"></td>
					<td><input class="modify" type="text" name="name"></td>
					<td><input class="modify" type="checkbox" name="is-hourly-paid" id="tick-hourly-paid" value="Yes" onclick="checkbox_clicked()" checked></td>
					<td><input class="modify" type="number" name="price" id="end-price" disabled></td>
					<td><input class="modify" type="number" name="hourly-pay" id="hourly-pay-amount"></td>
					<td><input class="modify" type="text" name="client"></td>
					<td><input class="modify" type="date" name="date" value="<?php echo $today ?>"></td>
					<td class='center'><button class='table-button green' type='submit'>Lisää</button></td>
				</form>
			</tr>
		</tbody>
	</table>
</div>
<div class="main-bottom-wrapper">
	<table class="table">
		<thead>
			<tr>
				<th>Laskunumero</th>
				<th>Tilanne</th>
				<th>Projektin nimi</th>
				<th>Loppuhinta (ALV 0%)</th>
				<th>Tuntihinta</th>
				<th>Yhteistuntimäärä</th>
				<th>Käyttäjä 1 tunnit</th>
				<th>Käyttäjä 2 tunnit</th>
				<th>Käyttäjä 3 tunnit</th>
				<th>Asiakas</th>
				<th>Aloituspäivämäärä</th>
				<th>Toimituspäivämäärä</th>
				<th>Toiminnot</th>
			</tr>
		</thead>
		<tbody>
			<?php
				$sql1 = "SELECT project_situation, project_number, project_name, project_price, is_hourly_paid, project_hourly_pay, project_client, project_start_date, project_delivery_date FROM project_table ORDER BY project_number DESC";
				$result1 = mysqli_query($conn, $sql1) or die(mysqli_error());;
				while($row = mysqli_fetch_assoc($result1)) {
					$decimal_price = str_replace('.', ',', number_format($row['project_price'], 2));
					if($row['is_hourly_paid'] == 1) {
						$result_user_1 = 0;
						$result_user_2 = 0;
						$result_user_3 = 0;
						$sql2 = "SELECT date_user_1_hours, date_user_2_hours, date_user_3_hours FROM date_table WHERE date_project_number = '{$row['project_number']}'";
						$result2 = mysqli_query($conn, $sql2) or die(mysqli_error());;
						while($row2 = mysqli_fetch_assoc($result2)) {
							$result_user_1 += $row2['date_user_1_hours'];
							$result_user_2 += $row2['date_user_2_hours'];
							$result_user_3 += $row2['date_user_3_hours'];
						}
						$result_overall = $result_user_1 + $result_user_2 + $result_user_3;

						$project_situation = $row['project_situation'];
						$class = "";
						if($project_situation == "Keskeneräinen") {
							echo "
								<tr>
									<td class='unfinished'>{$row['project_number']}</td>

									<form action='./includes/complete.php' method='GET'>
										<td class='unfinished'>
											{$row['project_situation']}
											<button class='complete-button' type='submit'>✔️</button>
											<input type='hidden' name='id' value='{$row['project_number']}'>
										</td>
									</form>

									<td class='unfinished'>{$row['project_name']}</td>
									<td class='unfinished'> " . str_replace('.', ',', number_format($result_overall * $row['project_hourly_pay'], 2)) . " €</td>
									<td class='unfinished'>" . str_replace('.', ',', number_format($row['project_hourly_pay'], 2)) . " €</td>
									<td class='unfinished'>$result_overall h</td>
									<td class='unfinished'>$result_user_1 h</td>
									<td class='unfinished'>$result_user_2 h</td>
									<td class='unfinished'>$result_user_3 h</td>
									<td class='unfinished'>{$row['project_client']}</td>
									<td class='unfinished'>" . date_format(new DateTime($row['project_start_date']), 'd.m.Y') . "</td>
									<td class='unfinished'>" . date_format(new DateTime($row['project_delivery_date']), 'd.m.Y') . "</td>
									<td class='unfinished center'><a class='table-button' href='./project.php?id={$row['project_number']}' type='submit'>Avaa</a></td>
								</tr>
							";
						}
						else if($project_situation == "Laskutettu") {
							echo "
								<tr>
									<td class='invoiced'>{$row['project_number']}</td>

									<td class='invoiced'>{$row['project_situation']}</td>

									<td class='invoiced'>{$row['project_name']}</td>
									<td class='invoiced'> " . str_replace('.', ',', number_format($result_overall * $row['project_hourly_pay'], 2)) . " €</td>
									<td class='invoiced'>" . str_replace('.', ',', number_format($row['project_hourly_pay'], 2)) . " €</td>
									<td class='invoiced'>$result_overall h</td>
									<td class='invoiced'>$result_user_1 h</td>
									<td class='invoiced'>$result_user_2 h</td>
									<td class='invoiced'>$result_user_3 h</td>
									<td class='invoiced'>{$row['project_client']}</td>
									<td class='invoiced'>" . date_format(new DateTime($row['project_start_date']), 'd.m.Y') . "</td>
									<td class='invoiced'>" . date_format(new DateTime($row['project_delivery_date']), 'd.m.Y') . "</td>
									<td class='invoiced center'><a class='table-button' href='./project.php?id={$row['project_number']}' type='submit'>Avaa</a></td>
								</tr>
							";
						}

						$new_price = $result_overall * $row['project_hourly_pay'];
						$sql3 = "UPDATE project_table SET project_price = '" . $new_price . "' WHERE project_number = '{$row['project_number']}'";
						mysqli_query($conn, $sql3);
					}
					else {
						$project_situation = $row['project_situation'];
						$class = "";
						if($project_situation == "Keskeneräinen") {
							$class = "unfinished";
						}
						else if($project_situation == "Laskutettu") {
							$class = "invoiced";
						}
						echo "
							<tr>
								<td class='{$class}'>{$row['project_number']}</td>

								<td class='{$class}'>{$row['project_situation']}</td>

								<td class='{$class}'>{$row['project_name']}</td>
								<td class='{$class}'>$decimal_price €</td>
								<td class='{$class}'>-</td>
								<td class='{$class}'>-</td>
								<td class='{$class}'>-</td>
								<td class='{$class}'>-</td>
								<td class='{$class}'>-</td>
								<td class='{$class}'>{$row['project_client']}</td>
								<td class='{$class}'>" . date_format(new DateTime($row['project_start_date']), 'd.m.Y') . "</td>
								<td class='{$class}'>" . date_format(new DateTime($row['project_delivery_date']), 'd.m.Y') . "</td>
								<td class='{$class}'> </td>
							</tr>
						";
					}
				}
			?>
		</tbody>
	</table>
</div>
...
